import React, { useEffect, useState, useCallback } from "react";
import {
  Table,
  Button,
  Space,
  notification,
  Popconfirm,
  Typography,
  Tag,
  Modal,
  Form,
  Input,
  Card,
} from "antd";
import {
  EditOutlined,
  DeleteOutlined,
  PlusOutlined,
  SaveOutlined,
  CloseOutlined,
} from "@ant-design/icons";
import { useCharacteristicStore } from "./characteristicStore";
import {
  Characteristic,
  CreateCharacteristicRequest,
} from "./CharacteristicModel";
import { apiClient } from "../../common/apiClient";
import { handleFormValidationErrors } from "../../common/formErrorHandler";

interface CharacteristicDashboardProps {
  characteristicTypeId: number;
}

const CharacteristicDashboard: React.FC<CharacteristicDashboardProps> = ({
  characteristicTypeId,
}) => {
  const [editingKey, setEditingKey] = useState<number | null>(null);
  const [isCreateModalVisible, setIsCreateModalVisible] = useState(false);
  const [form] = Form.useForm();
  const [createForm] = Form.useForm();

  const {
    characteristicList,
    loading,
    totalCount,
    currentPage,
    pageSize,
    setPagination,
    setLoading,
    setTotalCount,
    setCharacteristicList,
    setCurrentPage,
  } = useCharacteristicStore();

  // Load characteristics
  const loadCharacteristics = useCallback(async () => {
    setLoading(true);
    apiClient.get<Characteristic[]>(
      `/api/characteristic-type/${characteristicTypeId}/characteristic`,
      {
        onSuccess: (data) => {
          setTotalCount(data?.length || 0);
          setCharacteristicList(data || []);
        },
        onServerError: () => {
          setCharacteristicList([]);
          notification.error({ message: "Failed to load characteristics!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      }
    );
  }, [setLoading, setTotalCount, setCharacteristicList, characteristicTypeId]);

  useEffect(() => {
    loadCharacteristics();
  }, [loadCharacteristics]);

  // Inline editing functions
  const isEditing = (record: Characteristic) => record.id === editingKey;

  const edit = (record: Characteristic) => {
    form.setFieldsValue({
      name: record.name,
      code: record.code,
      value: record.value,
    });
    setEditingKey(record.id);
  };

  const cancel = () => {
    setEditingKey(null);
  };

  const save = async (id: number) => {
    try {
      const row = await form.validateFields();
      const updatedData = { ...row, id };

      setLoading(true);
      apiClient.put<boolean>(`/api/characteristic`, updatedData, {
        onSuccess: () => {
          notification.success({
            message: "Characteristic updated successfully!",
          });
          setEditingKey(null);
          loadCharacteristics();
        },
        onValidationError: (errors: Record<string, string[]>) => {
          handleFormValidationErrors({
            form,
            errors,
          });
        },
        onServerError: () => {
          notification.error({
            message: "Failed to update characteristic!",
          });
        },
        onFinally: () => {
          setLoading(false);
        },
      });
    } catch (errInfo) {
      console.log("Validate Failed:", errInfo);
    }
  };

  // Create new characteristic
  const handleCreate = async (values: CreateCharacteristicRequest) => {
    setLoading(true);
    const createData = {
      ...values,
      characteristicTypeId: characteristicTypeId,
    };
    apiClient.post<number>(`/api/characteristic`, createData, {
      onSuccess: () => {
        notification.success({
          message: "Characteristic created successfully!",
        });
        setIsCreateModalVisible(false);
        createForm.resetFields();
        loadCharacteristics();
      },
      onValidationError: (errors: Record<string, string[]>) => {
        handleFormValidationErrors({
          form: createForm,
          errors,
        });
      },
      onServerError: () => {
        notification.error({
          message: "Failed to create characteristic!",
        });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  // Delete characteristic
  const handleDelete = async (id: number) => {
    setLoading(true);
    apiClient.delete<boolean>(`/api/characteristic/${id}`, undefined, {
      onSuccess: () => {
        const newTotalCount = totalCount - 1;
        const lastPage = Math.ceil(newTotalCount / pageSize);
        if (currentPage > lastPage && lastPage > 0) {
          setCurrentPage(lastPage);
        }
        notification.success({
          message: "Characteristic deleted successfully!",
        });
        loadCharacteristics();
      },
      onServerError: () => {
        notification.error({
          message: "Failed to delete characteristic!",
        });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  const handlePaginationChange = (page: number, pageSize: number) => {
    setPagination(page, pageSize);
  };

  // Table columns with inline editing
  const columns = [
    {
      title: "Name",
      dataIndex: "name",
      key: "name",
      render: (text: string, record: Characteristic) => {
        if (isEditing(record)) {
          return (
            <Form.Item
              name="name"
              style={{ margin: 0 }}
              rules={[{ required: true, message: "Please input name!" }]}
            >
              <Input />
            </Form.Item>
          );
        }
        return text;
      },
    },
    {
      title: "Code",
      dataIndex: "code",
      key: "code",
      render: (text: string, record: Characteristic) => {
        if (isEditing(record)) {
          return (
            <Form.Item
              name="code"
              style={{ margin: 0 }}
              rules={[{ required: true, message: "Please input code!" }]}
            >
              <Input />
            </Form.Item>
          );
        }
        return text;
      },
    },
    {
      title: "Value",
      dataIndex: "value",
      key: "value",
      render: (text: string, record: Characteristic) => {
        if (isEditing(record)) {
          return (
            <Form.Item
              name="value"
              style={{ margin: 0 }}
              rules={[{ required: true, message: "Please input value!" }]}
            >
              <Input />
            </Form.Item>
          );
        }
        return text;
      },
    },
    {
      title: "Status",
      dataIndex: "isActive",
      key: "isActive",
      render: (isActive: boolean) => (
        <Tag color={isActive ? "green" : "red"}>
          {isActive ? "Active" : "Inactive"}
        </Tag>
      ),
    },
    {
      title: "Actions",
      key: "actions",
      render: (_: unknown, record: Characteristic) => {
        const isEdit = isEditing(record);
        return (
          <Space>
            {isEdit ? (
              <>
                <Button
                  type="link"
                  icon={<SaveOutlined />}
                  onClick={() => save(record.id)}
                  loading={loading}
                />
                <Button type="link" icon={<CloseOutlined />} onClick={cancel} />
              </>
            ) : (
              <>
                <Button
                  type="link"
                  icon={<EditOutlined />}
                  onClick={() => edit(record)}
                  disabled={editingKey !== null}
                />
                <Popconfirm
                  title="Are you sure to delete this characteristic?"
                  onConfirm={() => handleDelete(record.id)}
                >
                  <Button type="link" icon={<DeleteOutlined />} />
                </Popconfirm>
              </>
            )}
          </Space>
        );
      },
    },
  ];

  return (
    <Card title="Characteristics" style={{ marginTop: 16 }}>
      <Space
        className="mb-4"
        style={{ width: "100%", justifyContent: "space-between" }}
      >
        <Typography.Text>
          Manage characteristics for this characteristic type
        </Typography.Text>
        <Button
          type="primary"
          icon={<PlusOutlined />}
          onClick={() => setIsCreateModalVisible(true)}
        >
          Add Characteristic
        </Button>
      </Space>

      <Form form={form} component={false}>
        <Table
          columns={columns}
          dataSource={characteristicList}
          loading={loading}
          pagination={{
            current: currentPage,
            pageSize,
            total: totalCount,
            onChange: handlePaginationChange,
            size: "small",
          }}
          rowKey="id"
          scroll={{ x: 800 }}
          size="small"
        />
      </Form>

      {/* Create Modal */}
      <Modal
        title="Add New Characteristic"
        open={isCreateModalVisible}
        onCancel={() => {
          setIsCreateModalVisible(false);
          createForm.resetFields();
        }}
        footer={null}
        width={500}
      >
        <Form
          form={createForm}
          layout="vertical"
          onFinish={handleCreate}
          initialValues={{
            characteristicTypeId: characteristicTypeId,
          }}
        >
          <Form.Item
            name="name"
            label="Name"
            rules={[{ required: true, message: "Please input name!" }]}
          >
            <Input placeholder="Enter name" />
          </Form.Item>

          <Form.Item
            name="code"
            label="Code"
            rules={[{ required: true, message: "Please input code!" }]}
          >
            <Input placeholder="Enter code" />
          </Form.Item>

          <Form.Item
            name="value"
            label="Value"
            rules={[{ required: true, message: "Please input value!" }]}
          >
            <Input placeholder="Enter value" />
          </Form.Item>

          <Form.Item>
            <Space>
              <Button type="primary" htmlType="submit" loading={loading}>
                Create
              </Button>
              <Button
                onClick={() => {
                  setIsCreateModalVisible(false);
                  createForm.resetFields();
                }}
              >
                Cancel
              </Button>
            </Space>
          </Form.Item>
        </Form>
      </Modal>
    </Card>
  );
};

export default CharacteristicDashboard;
