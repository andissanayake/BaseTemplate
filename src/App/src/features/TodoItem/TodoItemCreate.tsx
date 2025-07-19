import React, { useEffect } from "react";
import { Modal, Form, Input, DatePicker, Select, notification } from "antd";
import { useTodoItemStore } from "./todoItemStore";
import { apiClient } from "../../common/apiClient";
import dayjs from "dayjs";
import { useParams } from "react-router-dom";
import { TodoItem, PriorityLevel } from "./TodoItemModel";
import { handleFormValidationErrors } from "../../common/formErrorHandler";

interface TodoItemCreateProps {
  visible: boolean;
  onClose: () => void;
  refresh: () => void;
}

const TodoItemCreate: React.FC<TodoItemCreateProps> = ({
  visible,
  onClose,
  refresh,
}) => {
  const [form] = Form.useForm();
  const { setLoading } = useTodoItemStore();
  const { listId } = useParams();

  const getPriorityLevels = () => {
    const priorityOptions = [
      { label: "None", value: PriorityLevel.None },
      { label: "Low", value: PriorityLevel.Low },
      { label: "Medium", value: PriorityLevel.Medium },
      { label: "High", value: PriorityLevel.High },
    ];
    return priorityOptions;
  };

  const handleSave = async () => {
    form.validateFields().then(async (values) => {
      const payload: TodoItem = {
        ...values,
        reminder: values.reminder ? dayjs(values.reminder).format() : null,
        listId: +listId!,
      };
      setLoading(true);
      apiClient.post<number>("/api/todoItems", payload, {
        onSuccess: () => {
          notification.success({ message: "Todo item created successfully!" });
          refresh();
          onClose();
        },
        onValidationError: (errors) => {
          handleFormValidationErrors({
            form,
            errors,
          });
        },
        onServerError: () => {
          notification.error({ message: "Failed to create todo item!" });
        },
        onFinally: () => {
          setLoading(false);
        },
      });
    });
  };
  useEffect(() => {
    if (visible) {
      form.resetFields();
    }
  }, [visible, form]);

  return (
    <Modal
      title="Create Todo Item"
      open={visible}
      onCancel={onClose}
      onOk={handleSave}
    >
      <Form form={form} layout="vertical">
        <Form.Item label="Title" name="title" rules={[{ required: true }]}>
          <Input />
        </Form.Item>
        <Form.Item label="Note" name="note">
          <Input.TextArea rows={3} />
        </Form.Item>
        <Form.Item label="Reminder" name="reminder">
          <DatePicker
            showTime
            format="YYYY-MM-DD HH:mm"
            style={{ width: "100%" }}
          />
        </Form.Item>
        <Form.Item label="Priority" name="priority">
          <Select options={getPriorityLevels()} />
        </Form.Item>
      </Form>
    </Modal>
  );
};

export default TodoItemCreate;
