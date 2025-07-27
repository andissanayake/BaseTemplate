import React, { useEffect, useCallback } from "react";
import {
  Tree,
  Button,
  Space,
  notification,
  Popconfirm,
  Typography,
  Card,
  Spin,
} from "antd";
import {
  EditOutlined,
  DeleteOutlined,
  PlusOutlined,
  AppstoreOutlined,
  EyeOutlined,
} from "@ant-design/icons";
import type { DataNode } from "antd/es/tree";
import { useSpecificationStore } from "./specificationStore";
import { SpecificationModel } from "./SpecificationModel";
import { apiClient } from "../../common/apiClient";
import { Link } from "react-router-dom";

const { Title } = Typography;

const SpecificationList: React.FC = () => {
  const { specifications, loading, setSpecifications, setLoading } =
    useSpecificationStore();

  const loadSpecifications = useCallback(async () => {
    setLoading(true);
    apiClient.get<{ specifications: SpecificationModel[] }>(
      "/api/specifications",
      {
        onSuccess: (data) => {
          setSpecifications(data?.specifications || []);
        },
        onServerError: () => {
          setSpecifications([]);
          notification.error({ message: "Failed to load specifications!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      }
    );
  }, [setSpecifications, setLoading]);

  useEffect(() => {
    loadSpecifications();
  }, [loadSpecifications]);

  const handleDelete = async (id: number) => {
    setLoading(true);
    apiClient.delete<boolean>(`/api/specifications/${id}`, undefined, {
      onSuccess: () => {
        notification.success({
          message: "Specification deleted successfully!",
        });
        loadSpecifications();
      },
      onServerError: () => {
        notification.error({ message: "Failed to delete specification!" });
      },
      onFinally: () => {
        setLoading(false);
      },
    });
  };

  const convertToTreeData = (specs: SpecificationModel[]): DataNode[] => {
    return specs.map((spec) => ({
      key: spec.id.toString(),
      title: (
        <Space>
          <AppstoreOutlined />
          <span>{spec.name}</span>
          <Space size="small">
            <Link to={`/specifications/view/${spec.id}`}>
              <Button type="text" size="small" icon={<EyeOutlined />} />
            </Link>
            <Link to={`/specifications/edit/${spec.id}`}>
              <Button type="text" size="small" icon={<EditOutlined />} />
            </Link>
            <Popconfirm
              title="Delete specification"
              description="Are you sure you want to delete this specification?"
              onConfirm={() => handleDelete(spec.id)}
              okText="Yes"
              cancelText="No"
            >
              <Button
                type="text"
                size="small"
                danger
                icon={<DeleteOutlined />}
              />
            </Popconfirm>
          </Space>
        </Space>
      ),
      children:
        spec.children.length > 0 ? convertToTreeData(spec.children) : undefined,
    }));
  };

  const treeData = convertToTreeData(specifications);

  return (
    <Card>
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: 16,
        }}
      >
        <Title level={3} style={{ margin: 0 }}>
          Specifications
        </Title>
        <Link to="/specifications/create">
          <Button type="primary" icon={<PlusOutlined />}>
            Add Specification
          </Button>
        </Link>
      </div>

      <Spin spinning={loading}>
        <Tree showLine showIcon treeData={treeData} defaultExpandAll={true} />
      </Spin>
    </Card>
  );
};

export default SpecificationList;
